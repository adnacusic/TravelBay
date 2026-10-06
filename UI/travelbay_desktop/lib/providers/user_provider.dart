import '../models/user.dart';
import '../models/user_stats.dart';
import 'base_provider.dart';

class UserProvider extends BaseProvider<User> {
  UserProvider() : super('Users');

  @override
  User fromJson(Map<String, dynamic> json) => User.fromJson(json);

  Future<UserStats> getStats() async =>
      UserStats.fromJson(await getJson('$endpoint/Stats'));

  Future<UserActivity> getActivity(int id) async =>
      UserActivity.fromJson(await getJson('$endpoint/$id/Activity'));

  Future<User> activate(int id) async =>
      fromJson((await sendAction('PUT', '$endpoint/$id/Activate'))!);

  Future<User> deactivate(int id) async =>
      fromJson((await sendAction('PUT', '$endpoint/$id/Deactivate'))!);

  Future<void> resetPassword(
    int id, {
    required String newPassword,
    required String confirmNewPassword,
  }) =>
      sendAction('PUT', '$endpoint/$id/ResetPassword', {
        'newPassword': newPassword,
        'confirmNewPassword': confirmNewPassword,
      });

  Future<User> getMe() async => fromJson(await getJson('$endpoint/Me'));

  Future<User> updateMe(Map<String, dynamic> request) async =>
      fromJson((await sendAction('PUT', '$endpoint/Me', request))!);

  Future<void> changePassword({
    required String password,
    required String newPassword,
    required String confirmNewPassword,
  }) =>
      sendAction('PUT', '$endpoint/Me/ChangePassword', {
        'password': password,
        'newPassword': newPassword,
        'confirmNewPassword': confirmNewPassword,
      });
}
